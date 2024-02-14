import * as crypto from "crypto";
import * as fs from "fs";
import * as path from "path";

export function getFileHash(filename: string): string {
    const data = fs.readFileSync(path.join(__dirname, filename), {encoding: "utf8"});
    const hash = crypto.createHash("md5").update(data, "utf8");
    return hash.digest("hex");
}