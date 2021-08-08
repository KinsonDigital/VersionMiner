import {existsSync} from "fs";
import {FileLoader} from "../fileLoader";
import {ActionInputs} from "../interfaces/actionInputs";

/**
 * Represents the environment.
 */
export class Environment {
	/* eslint-disable @typescript-eslint/lines-between-class-members */
	private fileLoader: FileLoader;
	private inputs: ActionInputs;
	/* eslint-enable @typescript-eslint/lines-between-class-members */

	/**
     * Creates a new instance of Environment.
     */
	constructor () {
    	this.fileLoader = new FileLoader();
		let fileData: string = "";

		// The env.json file will not exist in production and is not
		// committed to the repository.		
		if (existsSync("./env.json")) {
			fileData = this.fileLoader.loadEnvFile("./env.json");
			this.inputs = JSON.parse(fileData);
		} else {
			// This branch only runs if a production version.
			// Set to environment value of production			
			this.inputs = {
				environment: "production",
				repoOwner: "",
				repoName: "",
				relativeFilePath: "",
				userName: "",
				password: "",
			};
		}
	}
	
	/**
     * Returns a value indicating if in a production environment.
     * @returns True if the environment is production.
     */
	public isProd (): boolean {
		/* eslint-disable @typescript-eslint/no-unsafe-member-access */
		/* eslint-disable @typescript-eslint/indent */
    	for (const [key, value, ] of Object.entries(this.inputs)) {
    		if (key === "environment") {
    			switch (value.toString().toLowerCase()) {
    			case "prod":
    			case "production":
    			case "":
				case null:
    			case undefined:
    				return true;
    			default:
    				return false;
    			}
    		}
    	}

    	return false;
		/* eslint-enable @typescript-eslint/no-unsafe-member-access */
		/* eslint-enable @typescript-eslint/indent */
	}

	/**
     * Returns a value indicating if in a development environment.
     * @returns True if the environment is development.
     */
	public isDevelop (): boolean {
		/* eslint-disable @typescript-eslint/no-unsafe-member-access */
    	for (const [key, value, ] of Object.entries(this.inputs)) {
    		if (key === "environment") {
    			let stringValue: string = value.toString().toLowerCase();
    			return stringValue === "dev" || stringValue === "develop";
    		}
    	}

    	return false;
		/* eslint-enable @typescript-eslint/no-unsafe-member-access */
	}

	/**
     * Returns the value of a variable that matches the given name.
     * @param varName The name of the variable.
     * @returns The value of the given variable.
     */
	public getVarValue (varName: string, throwErrorWhenNotFound: boolean = true): string {
    	for (const [key, value, ] of Object.entries(this.inputs)) {
    		if (key === varName) {
    			return value;
    		}
    	}

		if (throwErrorWhenNotFound) {
			throw new Error(`Could not find the environment variable '${varName}'.`);
		}

		return "";
	}
}