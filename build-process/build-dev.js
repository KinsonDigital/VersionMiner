import { existsSync } from "fs";
import { execSync } from "child_process";
import { stdout } from "process";

let nodeModulesPath = `${process.cwd()}\\node_modules`;

// First check if the node_modules folder exists.
// If not, pull the npm packages before the build
if (existsSync(nodeModulesPath) === false) {
    stdout.write("\nPulling npm packages . . . ");

    execSync("yarn install", { cwd: process.cwd() },
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

    stdout.write("Pull complete");
}


stdout.write("\nBuilding GitHub Action . . . ");

execSync("tsc --outDir bin", { cwd: process.cwd() },
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

process.stdout.write("Build Complete\n");
