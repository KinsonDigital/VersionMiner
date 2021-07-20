/**
 * Represents environment variables of the application.
 */
export interface EnvironmentVars {
	/**
     * The current environment.
     * @summary The values 'dev' and 'develop' are valid values for the development environment.
     * The values 'prod', 'production', undefined, or empty all represents the production environment.
     * This value is not case sensitive.
     */
	environment: string,
	/**
     * The owner of the repository.
     */
	repoOwner: string,
	/**
     * The name of the repository.
     */
	repoName: string,
	/**
     * The path to the file to download relative to the root of the repository.
     */
	relativeFilePath: string,
	/**
     * The user name to authenticate to the repository.
     */
	userName: string,
	/**
     * The password to authenticate to the repository.
     */
	password: string    
}