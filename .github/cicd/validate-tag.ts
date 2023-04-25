import { RunnerService } from "./RunnerService.ts";

// Validate the arguments
if (Deno.args.length !== 2) {
    let errorMsg = "The 'validate-tag' cicd script must have two arguments.";
    errorMsg += "\nThe first arg must be either 'production' or 'preview'.";
    errorMsg += "\nThe second arg must be the name of the tag.";

    throw new Error(errorMsg);
}

const tagType: string = Deno.args[0].toLowerCase();

if (tagType !== "production" && tagType !== "preview") {
    let errorMsg = "The tag type argument must be a value of 'production' or 'preview'.";
    errorMsg += "\nThe value is case-insensitive.";

    throw new Error(errorMsg);
}

const prodVersionRegex = /^v[0-9]+\.[0-9]+\.[0-9]+$/;
const prevVersionRegex = /^v[0-9]+\.[0-9]+\.[0-9]+-preview\.[0-9]+$/;

const tag: string = Deno.args[1];
let isValid = false;

if (tagType === "production") {
    isValid = prodVersionRegex.test(tag);
} else {
    isValid = prevVersionRegex.test(tag);
}

if (isValid === false) {
    throw new Error(`The tag is not in the correct ${tagType} version syntax.`);
}

const runnerService: RunnerService = new RunnerService();
const result: [boolean, string] = await runnerService.run(["git", "tag"]);

if (result[0] === false) {
    let errorMsg = "The command 'git tag' failed.";
    errorMsg += `\n${result[1]}`;

    throw new Error(errorMsg);
}

const tags: string[] = result[1].split("\n");

const tagExists: boolean = tags.some(t => t === tag);

if (tagExists) {
    throw new Error(`The tag '${tag}' already exists.`);
}
