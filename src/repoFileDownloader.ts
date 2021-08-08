import {info} from "@actions/core";
import axios, {AxiosRequestConfig, AxiosResponse} from "axios";

/**
 * Downloads the contents of a file from a branch from a repository.
 */
export class RepoFileDownloader {
	/**
	 * Downloads the contents of a file.
	 * @param repoOwnerAndName The owner of the repository and name of the repository separated by a forward slash.
	 * @param branch The name of the branch.
	 * @param relativeFilePath The path to the file to download relative to the root of the repository.
	 * @param githubToken The token  to authenticate to the repository.
	 * @returns The string contents of the file.
	 */
	public async downloadFile (repoOwnerAndName: string,
							   branch: string,
							   relativeFilePath: string,
							   githubToken: string): Promise<string> {
		
		if (this.isInvalidString(repoOwnerAndName)) {
			throw new Error("The 'repoOwnerAndName' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(branch)) {
			throw new Error("The 'branch' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(relativeFilePath)) {
			throw new Error("The 'relativeFilePath' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(githubToken)) {
			throw new Error("The 'githubToken' param must not be null, empty, or undefined.");
		}
		
		const config: AxiosRequestConfig = {
			baseURL: "https://api.github.com",
			headers: {
				"Accept": "application/vnd.github.v3.raw",
				"Authorization" : `token ${githubToken}`,
			},
		};
        
		const url: string = `/repos/${repoOwnerAndName}/contents/${relativeFilePath}?ref=${branch}`;

		info("URL used to download file from repository:");
		info(`\n${url}`);

		try {
			const response: AxiosResponse<string> = await axios.get<string>(url, config);

			return await Promise.resolve(response.data);
		} catch (error) {
			/* eslint-disable @typescript-eslint/no-unsafe-member-access */
			const res: AxiosResponse = <AxiosResponse>error.response;

			throw new Error(`${res.status} - ${res.statusText} - ${res.data.message}`);
			/* eslint-enable @typescript-eslint/no-unsafe-member-access */
		}
	}

	/**
	 * Returns a value indicating if the string value is null, undefined, or empty.
	 * @param value The string value to validate.
	 * @returns True if the string is not null, undefined, or empty.
	 */
	private isInvalidString (value: string): boolean {
		return value === undefined ||
			value === null ||
			value === "";
	}
}