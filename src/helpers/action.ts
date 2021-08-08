import {getInput, setOutput, info, warning, setFailed, InputOptions} from "@actions/core";
import {Environment as Environment} from "./environment";

/**
 * Represents different action functionality.
 */
export class Action {
	private environment: Environment;

	private devEnvOutputs: Record<string, any> = {};

	/**
     * Creates a new instance of ActionInputs
     */
	constructor () {
    	this.environment = new Environment();
	}

	/**
     * Returns the value of the input that matches the given input.
     * @param name The name of the input.
     * @returns The value of the given input.
     */
	public getInput (name: string): string {
		// Development version pulls from the 'env.json' file for testing
		if (this.environment.isDevelop()) {
			let isRequired: boolean = true;

			if (name === "contains") {
				isRequired = false;
			}

			return this.environment.getVarValue(name, isRequired);
		} else if (this.environment.isProd()) {
			// Production version pulls the inputs from the YAML file
			let options: InputOptions = {
				required: true,
			};

			if (name === "contains") {
				options.required = false;
			}

			return getInput(name, options);
    	} else {
			throw new Error("Unknown environment.");
		}
	}

	/**
	 * Sets the value of the given output to the given value.
	 * @param name The name of the output.
	 * @param value The value of the output.
	 */
	public setOutput (name: string, value: string): void {
		if (this.environment.isDevelop()) {
			this.devEnvOutputs[name] = value;
		} else if (this.environment.isProd()) {
			setOutput(name, value);
		} else {
			throw new Error("Unknown environment.");
		}
	}

	/**
	 * Writes info to log with console.log. 
	 * @param message Info message.
	 */
	public info (message: string): void {
		if (this.environment.isDevelop()) {
			console.info(message);
		} else if (this.environment.isProd()) {
			info(message);
		} else {
			throw new Error("Unknown environment.");
		}
	}

	/**
	 * Adds a warning issue.
	 * @param message Warning issue message.  Errors will be converted to string via toString().
	 */
	public warning (message: string): void {
		if (this.environment.isDevelop()) {
			console.warn(message);
		} else if (this.environment.isProd()) {
			warning(message);
		} else {
			throw new Error("Unknown environment");
		}
	}

	/**
	 * Adds and error issue.
	 * @param message Error issue message.  Errors will be converted to string via toString().
	 */
	public setFailed (message: string | Error): void {
		if (this.environment.isDevelop()) {
			let errorMessage: string = "";
			const paramType: string = typeof message;

			if (paramType === "string") {
				errorMessage = <string>message;
			} else if (paramType === "Error") {
				errorMessage = (<Error>message).message;
			} else {
				errorMessage = "unknown error";
			}

			console.error(errorMessage);
		} else if (this.environment.isProd()) {
			setFailed(message);
		} else {
			throw new Error("Unknown environment.");
		}
	}
}