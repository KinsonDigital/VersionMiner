import { exists, existsSync } from "https://deno.land/std@0.184.0/fs/exists.ts";

// Validate the arguments
if (Deno.args.length !== 2) {
    let errorMsg = "The 'validate-notes' cicd script must have two arguments.";
    errorMsg += "\nThe first arg must be either 'production' or 'preview'.";
    errorMsg += "\nThe second arg must be the version of the notes.";

    throw new Error(errorMsg);
}

const notesType: string = Deno.args[0].toLowerCase();

if (notesType !== "production" && notesType !== "preview") {
    let errorMsg = "The notes type argument must be a value of 'production' or 'preview'.";
    errorMsg += "\nThe value is case-insensitive.";

    throw new Error(errorMsg);
}

const prodVersionRegex = /^v[0-9]+\.[0-9]+\.[0-9]+$/;
const prevVersionRegex = /^v[0-9]+\.[0-9]+\.[0-9]+-preview\.[0-9]+$/;

const version: string = Deno.args[1];
let isValid = false;

if (notesType === "production") {
    isValid = prodVersionRegex.test(version);
} else {
    isValid = prevVersionRegex.test(version);
}

if (isValid === false) {
    throw new Error(`The version is not in the correct ${notesType} version syntax.`);
}

const notesDirName = notesType === "production" ? "ProductionReleases" : "PreviewReleases";
const notesFilePath = `${Deno.cwd()}/Documentation/ReleaseNotes/${notesDirName}/Release-Notes-${version}.md`;

if (!existsSync(notesFilePath)) {
    throw new Error(`The release notes '${notesFilePath}' do not exist.`);
}
