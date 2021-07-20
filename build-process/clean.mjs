import { argv, stdout } from "process";
import { existsSync, rmSync, unlinkSync } from "fs";
import glob from "glob";
import cliProgress from "cli-progress";

let args = argv.slice(2);

if (args.length === 1) {
    let dirPath;
    let message;
    let dirName;

    // Get the base path which is the root of the entire project
    let basePath = process.cwd();

    // The possible arguments allowed
    switch (args[0]) {
        case "dist":
            dirPath = `${basePath}/dist`;
            dirName = "dist";
            message = "Deleting 'dist' directory . . . ";
            break;
        case "bin":
            dirPath = `${basePath}/bin`;
            dirName = "bin";
            message = "Deleting 'bin' directory . . . ";
            break;
        case "modules":
            dirPath = `${basePath}/node_modules`;
            dirName = "node_modules";
            message = "Deleting 'node_modules' directory . . . ";
            break;
        default:
            throw new Error(`Unknown clean command '${val}'`);
            break;
    }
    
    // Delete the directory if it exists
    if (existsSync(dirPath)) {
        console.log(`\n${message}`);

        let result = glob.sync(`${dirPath}/**/*.*`,
        { nodir: true, dot: true});

        const progressBar = new cliProgress.SingleBar({}, cliProgress.shades_classic);
        progressBar.start(result.length, 0);

        for (let i = 0; i < result.length; i++) {
            unlinkSync(result[i]); // Delete the file
            progressBar.update(i);
        }
        
        // Finish deleting the parent folder
        // This might have files with no extensions left over that was
        // not picked up by the glob npm package
        rmSync(dirPath,
            { recursive: true, force: true },
            (error) => {
                if (error) {
                    console.log(error);
                }
            });

        progressBar.update(result.length);
        progressBar.stop();

        stdout.write("Deletion complete\n");
    } else {
        console.log(`Deletion of directory '${dirName}' skipped.  Directory already deleted.`);
    }
}
