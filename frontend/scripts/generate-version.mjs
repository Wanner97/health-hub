import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const frontendRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(frontendRoot, '..');

const manifestPath = path.join(repoRoot, 'versions.json');
const generatedDirPath = path.join(frontendRoot, 'src', 'generated');
const generatedFilePath = path.join(generatedDirPath, 'version.generated.js');
const packageJsonPath = path.join(frontendRoot, 'package.json');
const packageLockPath = path.join(frontendRoot, 'package-lock.json');

const semVerPattern = /^\d+\.\d+\.\d+$/;

function readJsonFile(filePath) {
  if (!fs.existsSync(filePath)) {
    throw new Error(`File not found: ${filePath}`);
  }

  const rawContent = fs.readFileSync(filePath, 'utf8');
  return JSON.parse(rawContent);
}

function writeTextFile(filePath, content) {
  const directoryPath = path.dirname(filePath);
  fs.mkdirSync(directoryPath, { recursive: true });
  fs.writeFileSync(filePath, content, 'utf8');
}

const manifest = readJsonFile(manifestPath);

const suiteVersion = manifest?.suite?.version;
const frontendVersion = manifest?.services?.frontend?.version;

if (!suiteVersion || !semVerPattern.test(suiteVersion)) {
  throw new Error(`Invalid suite version in versions.json: ${suiteVersion}`);
}

if (!frontendVersion || !semVerPattern.test(frontendVersion)) {
  throw new Error(`Invalid frontend version in versions.json: ${frontendVersion}`);
}

const generatedFileContent =
  `export const SUITE_VERSION = ${JSON.stringify(suiteVersion)};\n` +
  `export const FRONTEND_VERSION = ${JSON.stringify(frontendVersion)};\n`;

writeTextFile(generatedFilePath, generatedFileContent);

const packageJson = readJsonFile(packageJsonPath);
packageJson.version = frontendVersion;

const updatedPackageJsonContent = `${JSON.stringify(packageJson, null, 2)}\n`;
writeTextFile(packageJsonPath, updatedPackageJsonContent);

if (fs.existsSync(packageLockPath)) {
  const packageLock = readJsonFile(packageLockPath);

  packageLock.version = frontendVersion;

  if (packageLock.packages && packageLock.packages['']) {
    packageLock.packages[''].version = frontendVersion;
  }

  const updatedPackageLockContent = `${JSON.stringify(packageLock, null, 2)}\n`;
  writeTextFile(packageLockPath, updatedPackageLockContent);
}

console.log(`Frontend version synced to ${frontendVersion}`);
console.log(`Suite version synced to ${suiteVersion}`);
console.log(`Generated file: ${generatedFilePath}`);
console.log(`Updated file: ${packageJsonPath}`);
console.log(`Updated file: ${packageLockPath}`);