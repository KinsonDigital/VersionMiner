import axios, {AxiosBasicCredentials, AxiosError, AxiosRequestConfig, AxiosResponse} from "axios";
import {RepoFile} from "./interfaces/repoFile";

/**
 * "main": "index.js",
 */
export class RepoFileDownloader {

	public async downloadFile (owner: string, repo: string, relativeFilePath: string, userName: string, password: string): Promise<string> {
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

		return await new Promise<string>((resolve, reject) => {
			axios.get<RepoFile>(url, config)
				.then((response: AxiosResponse<RepoFile>) => {
					resolve(response.data.toString());
				}, (error: AxiosError<Error>) => {
					reject(`${error.response?.status} - ${error.response?.statusText}`);
				});
		});
	}
}