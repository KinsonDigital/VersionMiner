import { existsSync } from "fs";
import { execSync } from "child_process";
import { stdout } from "process";

let nodeModulesPath = `${process.cwd()}\\node_modules`;

// First check if the node_modules folder exists.
// If not, pull the npm packages before the build
if (existsSync(nodeModulesPath) === false) {
    stdout.write("\nPulling npm packages . . . ");

    execSync("yarn install", { cwd: process.cwd() });

    stdout.write("Pull complete");
}

stdout.write("\nBuilding GitHub Action . . . ");

// Compile the typescript files to the bin folder
execSync("tsc --outDir bin", { cwd: process.cwd() });

process.stdout.write("Build Complete\n");
