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
			const repoOwner: string = actionInput.getInput("repo-owner");
			const repoName: string = actionInput.getInput("repo-name");
			const relativeFilePath: string = actionInput.getInput("relative-file-path");
			const userName: string = actionInput.getInput("user-name");
			const password: string = actionInput.getInput("password");
			
			const fileContent: string =
				await downloader.downloadFile(repoOwner,
					repoName,
					relativeFilePath,
					userName,
					password);
			
			const version: string = parser.getElementContent(fileContent, "<Version>", "</Version>");
			actionInput.setOutput("version", version);

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
		action.setFailed(error);
	});
