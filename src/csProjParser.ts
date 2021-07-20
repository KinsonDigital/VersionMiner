import {Action} from "./helpers/action";

/**
 * Parses csproj files.
 */
export class CSProjParser {
	private action: Action;

	/**
	 * Creates a new instance of CSProjParser.
	 */
	constructor () {
		this.action = new Action();
	}

	/**
	 * Gets the content between the given start and end tags.
	 * @param xmlContent The XML content to parse.
	 * @param startTag The start tag to search for.
	 * @param endTag The end tag to search for.
	 * @returns The content in between the start and end tags.
	 */
	public getElementContent (xmlContent: string, startTag: string, endTag: string): string {
		if (xmlContent === "" || xmlContent.length === 0) {
			throw new Error("The parameter 'htmlContent' must not be null or empty.");
		}

		if (startTag === "" || startTag.length === 0) {
			throw new Error("The parameter 'startTag' must not be null or empty.");
		}

		if (endTag === "" || endTag.length === 0) {
			throw new Error("The parameter 'endTag' must not be null or empty.");
		}

		const startTagIndex: number = xmlContent.indexOf(startTag);

		if (startTagIndex === -1) {
			throw new Error(`Could not find the start tag '${startTag}'`);
		}

		this.action.info(`The start tag '${startTag}' found.`);

		const endTagIndex: number = xmlContent.indexOf(endTag);
        
		if (endTagIndex === -1) {
			throw new Error(`Could not find the end tag '${endTag}'`);
		}

		this.action.info(`The start tag '${endTag}' found.`);

		const tagContent: string = xmlContent.substring(startTagIndex + startTag.length, endTagIndex);

		this.action.info(`Tag content '${tagContent}' found.`);
        
		return tagContent;
	}
}