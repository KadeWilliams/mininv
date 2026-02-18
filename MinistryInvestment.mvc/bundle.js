const fs = require("fs");
const path = require('path');
const esbuild = require('esbuild');

const imgSourceDirectory = 'node_modules/@hobbylobby/ui-toolkit-bootstrap/dist/img/logos/hobbylobby';
const imgOutputDirectory = 'wwwroot/img/toolkit';


cleanDirectory(imgOutputDirectory);

copyFiles(imgSourceDirectory, imgOutputDirectory, 'svg');
copyFiles(imgSourceDirectory, imgOutputDirectory, 'png');
copyFiles(imgSourceDirectory, imgOutputDirectory, 'ico');

bundleCss();
bundleJavaScript();
bundleOrganization();

function cleanDirectory(directory) {
    if (fs.existsSync(directory)) {
        fs.readdir(directory, (err, files) => {
            if (err) throw err;

            for (const file of files) {
                var filePath = path.join(directory, file);
                if (fs.lstatSync(filePath).isDirectory()) {
                    cleanDirectory(filePath);
                } else {
                    fs.unlinkSync(filePath);
                }
            }
        });
    }
}

function copyFiles(sourceDirectory, outputDirectory, extensionFilter) {
    if (!fs.existsSync(outputDirectory)) {
        fs.mkdirSync(outputDirectory, { recursive: true });
    }

    fs.readdir(sourceDirectory, (err, files) => {
        if (err) throw err;

        for (const file of files) {
            if (extensionFilter && file.split('.').pop() === extensionFilter) {
                fs.copyFile(path.join(sourceDirectory, file), path.join(outputDirectory, file), (err) => {
                    if (err) throw err;
                });
            }
            else if (!extensionFilter) {
                fs.copyFile(path.join(sourceDirectory, file), path.join(outputDirectory, file), (err) => {
                    if (err) throw err;
                });
            }
        }
    });
}

function bundleCss() {
    esbuild.build({
        logLevel: 'info',
        entryPoints: ['wwwroot/css/app.css'],
        bundle: true,
        minify: true,
        loader: {
            '.woff2': 'copy',
            '.ttf': 'copy'
        },
        outfile: 'wwwroot/css/app.bundle.css'
    }).catch(() => process.exit(1));
}

function bundleJavaScript() {
    esbuild.build({
        logLevel: 'info',
        entryPoints: ['wwwroot/js/app.js'],
        bundle: true,
        minify: true,
        sourcemap: true,
        outfile: 'wwwroot/js/app.bundle.js'
    }).catch(() => process.exit(1));
}

function bundleOrganization() {
    esbuild.build({
        logLevel: 'info',
        entryPoints: ['wwwroot/js/organization/index.js'],
        bundle: true,
        outfile: 'wwwroot/js/dist/organization.bundle.js',
        sourcemap: true,
        //format: 'iife',
        minify: true
    })
}
