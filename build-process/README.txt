The javascript files in the 'build-process' folder have the ".mjs" file extension instead of ".js".

The reason for this is because we cannot have the "type": "module" json key/value in the "package.json".
The reason we cannot have this in the "package.json" file is because for the "nodemon" package to work for
running the application for debugging purposes, the "type": "module" content cannot be in the "package.json" file.

To go around this, we can use the ".mjs" extension instead.  But because of this, we have to be explicit with
our node commands. With ".js" files, you do not have to use the extension in the command, but with ".mjs" files,
you do have to include the extension.

A ".mjs" file is a file that indicates to "Node" that the file is to use the "ES6" module system instead of the "CommonJs" one.

Basically, it will use the "import { obj } from 'library'" syntax instead of the "const obj = require("library")" syntax.

