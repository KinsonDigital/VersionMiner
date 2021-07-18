import { argv, stdout } from "process";
import { existsSync, rm, rmSync } from "fs";

let args = argv.slice(2);

if (args.length === 1) {
    let dirPath;
    let message;
    let dirName;
    
    // Get the base path which is the root of the entire project
    let basePath = process.cwd();

    switch (args[0]) {
        case "dist":
            dirPath = `${basePath}\\dist`;
            dirName = "dist";
            message = "Deleting 'dist' directory . . . ";
            break;
        case "bin":
            dirPath = `${basePath}\\bin`;
            dirName = "bin";
            message = "Deleting 'bin' directory . . . ";
            break;
        case "modules":
            dirPath = `${basePath}\\node_modules`;
            dirName = "node_modules";
            message = "Deleting 'node_modules' directory . . . ";
            break;
        default:
            console.error(`Unknown clean command '${val}'`);
            break;
    }
    
    // Delete the directory if it exists
    if (existsSync(dirPath)) {
        stdout.write(`\n${message}`);

        rmSync(dirPath,
            { recursive: true, force: true },
            (error) => {
                if (error) {
                    console.log(error);
                }
            });

        stdout.write("Deletion complete\n");
    } else {
        console.log(`Deletion of directory '${dirName}' skipped.  Directory already deleted.`);
    }
}
