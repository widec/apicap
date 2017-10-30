# apicap
API capture tool

Use this tool to capture your public API. Select the assemblies that you want to include in your public API and this tool creates and output file containing your public API. This file can be included in your repository and serve as a API history file. 

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/751e6d57cab447aeaa32796ecacba8c1)](https://www.codacy.com/app/widec/apicap?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=widec/apicap&amp;utm_campaign=Badge_Grade)

usage : ApiCap.CLI.exe filepattern -out filename

- filepattern : The path to the assemblies with wildcards. For example "c:\SomeProject\bin\debug\*.dll"
- filename : The output filename, this defaults to "output.txt"
