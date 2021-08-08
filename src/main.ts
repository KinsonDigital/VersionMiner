import {RepoFileDownloader} from "./repoFileDownloader";
import {Action} from "./helpers/action";
import {CSProjParser} from "./csProjParser";


/**
 * The main GitHub action.
 */
export class Application {
	public async main (): Promise<void> {
		const downloader: RepoFileDownloader = new RepoFileDownloader();
		const actionInput: Action = new Action();
		const parser: CSProjParser = new CSProjParser();
		
		try {
			const repoOwnerAndName: string = actionInput.getInput("repo-owner-and-name");
			const branch: string = actionInput.getInput("branch");
			const relativeFilePath: string = actionInput.getInput("relative-file-path");
			const githubToken: string = actionInput.getInput("github-token");
			const contains: string = actionInput.getInput("contains");

			if (!repoOwnerAndName.includes("/")) {
				throw new Error("The 'repo-owner-and-name' format must be a repository owner separated by a repository name.\n\t Example: JohnDoe/my-repo");
			}

			let fileContent: string = await downloader.downloadFile(repoOwnerAndName,
				branch,
				relativeFilePath,
				githubToken);
			
			const version: string = parser.getElementContent(fileContent, "<Version>", "</Version>");
			actionInput.setOutput("version", version);

			// If the version string does not contain the 'contains' input value, fail the action
			if (contains != "" && !version.includes(contains, 0)) {
				actionInput.setFailed(`The version '${version}' must contain the string '${contains}'`);
			}

			return await Promise.resolve();
		} catch (error) {
			throw error;
		}
	}
}

const app: Application = new Application();
const action: Action = new Action;

app.main()
	.then(() => {
		action.info("Action Success!!");
	}, (error: Error) => {
		action.setFailed(error.message);
	});
