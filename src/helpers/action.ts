import core from "@actions/core";
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
    	if (this.environment.isDevelop()) {
    		return this.environment.getVarValue(name);
		} else if (this.environment.isProd()) {
			try {
				return core.getInput(name);
			} catch (error) {
				const errorMsg: string = (<Error>error).message;

				if (errorMsg === "Cannot read property 'getInput' of undefined") {
					throw new Error("You are running locally with the environment set to production.\nMake sure your environment is set to development.");
				} else {
					throw new Error(errorMsg);
				}
			}
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
			try {
				core.setOutput(name, value);
			} catch (error) {
				const errorMsg: string = (<Error>error).message;

				if (errorMsg === "Cannot read property 'setOutput' of undefined") {
					throw new Error("You are running locally with the environment set to production.\nMake sure your environment is set to development.");
				} else {
					throw new Error(errorMsg);
				}
			}
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
			try {
				core.info(message);
			} catch (error) {
				const errorMsg: string = (<Error>error).message;

				if (errorMsg === "Cannot read property 'info' of undefined") {
					throw new Error("You are running locally with the environment set to production.\nMake sure your environment is set to development.");
				} else {
					throw new Error(errorMsg);
				}
			}
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
			core.warning(message);
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
			core.error(message);
		} else {
			throw new Error("Unknown environment.");
		}
	}
}