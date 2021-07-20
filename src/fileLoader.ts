import fs from "fs";

/**
 * Loads data from a file.
 */
export class FileLoader {
	/**
     * Loads the data from a file at the given file path.
     * @param filePath The path to the file to load.
     * @returns The file data.
     */
	public loadEnvFile (filePath: string): string {
		if (fs.existsSync(filePath)) {
			const rawData: Buffer = fs.readFileSync(filePath);

			return rawData.toString();
		}

		throw new Error(`Could find the file '${filePath}'.`);
	}
}
