IronSmalltalk Components (Assemblies) Overview:

* Runtime Components written in .Net:
	* Common (few classes shared between the compiler and the runtime)
	* Runtime (the core of IST; this is the brain and the heart)
	* Compiler (parses Smalltalk source code and creates ASTs)
	* Definition Installer (modifies the Smalltalk environment, e.g. creates clas, globals, methods etc.)
	* Interchange Installer (a definition installer that uses interchange files (*.ST files) to set up/modify the environment)
	* AST JIT Compiler (compiles ST ASTs to DLR ETs/Expressions)
	* Hosting (needed to host IST as language inside the DLR, but not necessary)

* Smalltalk Class Library
	* The standard class library - it doesn't need more introduction

* Development Tools
	* Class Hierarchy Browser / Editor (a tool written in .Net for the purpose of developing the class library - this is an internal dev. tool)
	* Test Playground (all type of experimental garbage)
	* Unit Tests