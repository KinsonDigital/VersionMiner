import axios, {AxiosBasicCredentials, AxiosRequestConfig, AxiosResponse} from "axios";

/**
 * "main": "index.js",
 */
export class RepoFileDownloader {

	public async downloadFile (owner: string,
							   repo: string,
							   relativeFilePath: string,
							   userName: string,
							   password: string): Promise<string> {
		
		if (this.isInvalidString(owner)) {
			throw new Error("The 'owner' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(repo)) {
			throw new Error("The 'repo' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(relativeFilePath)) {
			throw new Error("The 'relativeFilePath' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(userName)) {
			throw new Error("The 'userName' param must not be null, empty, or undefined.");
		}

		if (this.isInvalidString(password)) {
			throw new Error("The 'password' param must not be null, empty, or undefined.");
		}
		
		// NOTE: No authorization required due public repo, but API request
		// limit is lower with unauthorized requests.
		// https://docs.github.com/en/rest/overview/resources-in-the-rest-api#rate-limiting
		const creds: AxiosBasicCredentials = {
			username: userName,
			password: password,
		};

		const config: AxiosRequestConfig = {
			baseURL: "https://api.github.com",
			auth: creds,
			headers: {
				"Accept": "application/vnd.github.v3.raw",
				// "Authorization" : "token <token-here>"
			},
		};
        
		const url: string = `/repos/${owner}/${repo}/contents/${relativeFilePath}`;

		console.log(`URL: ${url}`);
		try {
			const response: AxiosResponse<string> = await axios.get<string>(url, config);

			return await Promise.resolve(response.data);
		} catch (error) {
			// eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
			const res: AxiosResponse = <AxiosResponse>error.response;

			// console.log(res);
			throw new Error(`${res.status} - ${res.statusText}`);
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