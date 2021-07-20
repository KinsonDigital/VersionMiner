import {FileLoader} from "../fileLoader";
import {EnvironmentVars} from "../interfaces/envVars";

/**
 * Represents the environment.
 */
export class Environment {
	/* eslint-disable @typescript-eslint/lines-between-class-members */
	private fileLoader: FileLoader;
	private vars: EnvironmentVars;
	/* eslint-enable @typescript-eslint/lines-between-class-members */

	/**
     * Creates a new instance of Environment.
     */
	constructor () {
    	this.fileLoader = new FileLoader();

    	const fileData: string = this.fileLoader.loadEnvFile("./env.json");

    	this.vars = JSON.parse(fileData);
	}
	
	/**
     * Returns a value indicating if in a production environment.
     * @returns True if the environment is production.
     */
	public isProd (): boolean {
		/* eslint-disable @typescript-eslint/no-unsafe-member-access */
		/* eslint-disable @typescript-eslint/indent */
    	for (const [key, value, ] of Object.entries(this.vars)) {
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
    	for (const [key, value, ] of Object.entries(this.vars)) {
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
	public getVarValue (varName: string): string {
    	for (const [key, value, ] of Object.entries(this.vars)) {
    		if (key === varName) {
    			return value;
    		}
    	}

    	throw new Error(`Could not find the environment variable '${varName}'.`);
	}
}