import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const ThisDir = path.dirname(fileURLToPath(import.meta.url));
const RootDir = path.resolve(ThisDir, "../../..");
const BundleDir = path.join(RootDir, "WebApps/WebApp/wwwroot/tp/js");

function loadBundle(FileName) {
    const FilePath = path.join(BundleDir, FileName);
    const Source = fs.readFileSync(FilePath, "utf8");
    window.eval(`${Source}\n//# sourceURL=${FilePath}`);
}

loadBundle("tp.js");
loadBundle("tp-Data.js");

globalThis.tp = window.tp;
