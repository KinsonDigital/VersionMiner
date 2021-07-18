import { existsSync } from "fs";
import { execSync } from "child_process";
import { stdout } from "process";

stdout.write("\nPublishing GitHub Action . . . ");

// STEP 1
// Clean the 'dist' directory
execSync("node build-process/clean \"dist\"",
    { cwd: process.cwd() },
    (error, stdout, stderr) => {
        if (error) {
            console.error(error);
            return;
        }

        if (stderr) {
            console.error(stderr);
            return;
        }

        console.log(stdout);
    });

// STEP 2
// Clean the 'node_modules' directory
execSync("node build-process/clean \"modules\"",
    { cwd: process.cwd() },
    (error, stdout, stderr) => {
        if (error) {
            console.error(error);
            return;
        }

        if (stderr) {
            console.error(stderr);
            return;
        }

        console.log(stdout);
    });

// STEP 3
// Pull down npm packages
stdout.write("\n\tPulling npm packages . . . ");

stdout.write("Pull complete");

execSync("yarn install",
    { cwd: process.cwd() },
    (error, stdout, stderr) => {
        if (error) {
            console.error(error);
            return;
        }
        
        if (stderr) {
            console.error(stderr);
            return;
        }
        
        console.log(stdout);
    });

// STEP 4
// Perform a single js library type build using the 'ncc' tool
execSync("ncc build src/main.ts --license licenses.txt",
    { cwd: process.cwd() },
    (error, stdout, stderr) => {
        if (error) {
            console.error(error);
            return;
        }
        
        if (stderr) {
            console.error(stderr);
            return;
        }
        
        console.log(stdout);
    });

console.log("\nPublish complete.");