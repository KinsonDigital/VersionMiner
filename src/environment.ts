import { FileLoader as FileLoader } from "./fileLoader";
import { EnvironmentVars } from "./envVars";

/**
 * Represents the environment.
 */
export class Environment {
    private fileLoader: FileLoader;
    private vars: EnvironmentVars;

    /**
     * Creates a new instance of Environment.
     */
    constructor() {
        this.fileLoader = new FileLoader();

        const fileData: string = this.fileLoader.loadEnvFile("./env.json");

        this.vars = JSON.parse(fileData);
    }

    /**
     * Returns a value indicating if in a production environment.
     * @returns True if the environment is production.
     */
    public isProd(): boolean {
        for (const [key, value] of Object.entries(this.vars)) {
            if (key === "environment") {
                switch (value.toString().toLowerCase()) {
                    case "prod":
                    case "production":
                    case "":
                    case undefined:
                        return true;
                    default:
                        return false;
                }
            }
        }

        return false;
    }

    /**
     * Returns a value indicating if in a development environment.
     * @returns True if the environment is development.
     */
    public isDevelop(): boolean {
        for (const [key, value] of Object.entries(this.vars)) {
            if (key === "environment") {
                let stringValue: string = value.toString().toLowerCase();
                return stringValue === "dev" || stringValue === "develop";
            }
        }

        return false;
    }

    /**
     * Returns the value of a variable that matches the given name.
     * @param varName The name of the variable.
     * @returns The value of the given variable.
     */
    public getVarValue(varName: string) {
        for (const [key, value] of Object.entries(this.vars)) {
            if (key === varName) {
                return value;
            }
        }

        throw new Error(`Could not find the environment variable '${varName}'.`);
    }
}