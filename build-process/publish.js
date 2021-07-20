import { execSync } from "child_process";
import { stdout } from "process";

stdout.write("\nPublishing GitHub Action . . . ");

// STEP 1
// Clean the 'dist' directory
execSync("node build-process/clean \"dist\"", { cwd: process.cwd() });

// STEP 2
// Clean the 'node_modules' directory
execSync("node build-process/clean \"modules\"", { cwd: process.cwd() });

// STEP 3
// Pull down npm packages
stdout.write("\nPulling npm packages . . . ");

stdout.write("Pull complete");

execSync("yarn install", { cwd: process.cwd() });

// STEP 4
// Perform a single js library type build using the 'ncc' tool
execSync("ncc build src/main.ts --license licenses.txt", { cwd: process.cwd() });

console.log("\nPublish complete.");
