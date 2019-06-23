# This is simple python script to deal with automatic sprite processing.
# It can be configured using a json file to do pretty much anything.
# It was pretty much only written for MechTransfer and it isn't really robust at all.
# If you need it for other things though, feel free to use it, I guess...

# Don't forget to install Pillow!

# The target ("t") can be:
#   The path and file name to save image
#   A folder name, file name will be the same as source
#   "$" no save
#   "$refname" no sace + explicit back ref name

# How to backrefs:
# The file path sould be replaced with: >SourceGroupName:BeckRefName
# Where back ref name is:
#   If the referenced image was saved (target[0] != $), then it's the name of the saved file.
#   If it wasn't and no explicit back ref name was given, then it's the name of the source file.
#   If there was an explit back ref name, then it's that.

# When saving an image to a folder that came from a back ref, the name of the file will be the original source file,
# or if it had an explicit back ref at some point, it will be the last explicit back ref name + the extention of the original souce file. 

import json
import os.path as p
from PIL import Image

def processContent(configPath, v):
    with open(configPath) as configFile:
         config = json.load(configFile)

    if v: print("Running preBatch script...")
    execTag(config, "preBatch")
    
    backRefs = {} # BackrefName -> (image, original base name, modified time)

    if v: print("Looking for backRefs...")
    for group in config["actionGroups"]:
        for file in group["files"]:
            if file["p"][0] == ">":
                backRefs[file["p"]] = None

    if v: print("Processing groups...")
    for group in config["actionGroups"]:
        if v: print("group: %s" % group["name"])

        inputFiles = [] # (image, target rel path, original base name, modified time)
        
        for file in group["files"]:

            # Grab image from backRef
            if file["p"][0] == ">":
                if backRefs[file["p"]] == None:
                    if v: print("   %s was not processed" % file["p"])
                else:
                    if v: print("   backRef: %s -> %s" % (file["p"], file["t"]))
                    ref = backRefs[file["p"]]
                    inputFiles.append((ref[0], file["t"], ref[1], ref[2]))

            # Open image from file
            else:
                fullpath = p.join(p.dirname(configPath), config["contentDir"], file["p"])
                backRefBaseName = p.basename(file["p"])

                if (v): print("   file: %s -> %s" % (fullpath, file["t"]))
                inputFiles.append((Image.open(fullpath), file["t"], backRefBaseName, p.getmtime(fullpath)))
		
        if group["action"][0] == ">":
            group["action"] = "def f(i): return " + group["action"][1:]
        execTag(group, "action")

        configModTime = p.getmtime(configPath)
        
        for (image, target, oldBaseName, modTime) in inputFiles:
            
            backRefName = ">" + group["name"] + ":"
            if target[0] != "$":
                savePath = p.join(p.dirname(configPath), config["targetDir"], target)
                if p.isdir(savePath):
                    savePath = p.join(savePath, oldBaseName)
                    newBaseName = oldBaseName
                else:
                    newBaseName = p.basename(target)
                backRefName = backRefName + newBaseName
            else:
                savePath = None
                if target == "$":
                    backRefName = backRefName + oldBaseName
                    newBaseName = oldBaseName
                else:
                    backRefName = backRefName + target[1:]
                    _, ext = p.splitext(oldBaseName)
                    newBaseName = target[1:] + ext

            if savePath != None and p.isfile(savePath):
                fileTime = p.getmtime(savePath)
            else:
                fileTime = None

            if fileTime != None and modTime <= fileTime and configModTime <= fileTime:
                print("   Up-to-date: %s" % savePath)
            else:

                # f() should be defined in action tag
                processed = f(image)

                if backRefName in backRefs:
                    backRefs[backRefName] = (processed, newBaseName, modTime)
            
                if savePath != None:
                    processed.save(savePath)
                    print("   Writing %s" % savePath)

    if v: print("Running postBatch script...")
    execTag(config, "postBatch")

def execTag(config, tag):
    exec(compile(fromFakeMultiline(config[tag]), "<" + tag + ">", "exec"), globals())

def fromFakeMultiline(text):
    if isinstance(text, list):
        return "\n".join(text)
    else:
        return text

def main():
    processContent("Content/content.json", True)

if (__name__ == "__main__"):
    main()
