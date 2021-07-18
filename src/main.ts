import { RepoFileDownloader } from "./repoFileDownloader";
import { Action } from "./action";
import { CSProjParser as CSProjParser } from "./csProjParser";

export class application {
	public async main(): Promise<void> {
		return new Promise<void>(async (_, reject) => {
			try {
				const downloader: RepoFileDownloader = new RepoFileDownloader();
				const actionInput: Action = new Action();
				
				const repoOwner = actionInput.getInput("repoOwner");
				const repoName = actionInput.getInput("repoName");
				const relativeFilePath = actionInput.getInput("relativeFilePath");
				const userName = actionInput.getInput("userName");
				const password = actionInput.getInput("password");

				const parser: CSProjParser = new CSProjParser();
				const fileContent: string = await downloader.downloadFile(repoOwner, repoName, relativeFilePath, userName, password);

				const version: string = parser.getElementContent(fileContent, "<Version>", "</Version>");
				actionInput.setOutput("version", version);
			} catch (error) {
				reject(error);
			}
		});
	}
}

const app: application = new application();
const action: Action = new Action;

app.main()
	.then(_ => {
		action.info("Action Success!!");
	}, (error: Error) => {
		action.setFailed(error);

		console.error(`Error: ${error.message}`);
	});
