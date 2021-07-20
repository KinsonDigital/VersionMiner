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
    		return core.getInput(name);
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
			core.setOutput(name, value);
		} else {
			throw new Error("Unknown environment.");
		}
	}

	/**
	 * Prints the given message to the logs.
	 * @param message The message to show in the logs.
	 */
	public info (message: string): void {
		if (this.environment.isDevelop()) {
			console.info(message);
		} else if (this.environment.isProd()) {
			core.info(message);
		} else {
			throw new Error("Unknown environment.");
		}
	}

	/**
	 * Fails the action with the given message.
	 * @param message The error message to fail the action with of type 'string' or 'Error'.
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