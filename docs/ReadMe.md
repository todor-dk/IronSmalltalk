
# IronSmalltalk solution structure #

### Folder Structure ###
The solution's folder structure is listed below.
``` 
\
   \bin          (1)
   \DLR          (2)
   \docs         (3)
   \obj          (4)
   \src          (5)
      \Core      (6)
      \Test      (7)
      \Tools     (8)
```

1. **bin** - artifacts directory, where the build output is located.
   This folder can be safely deleted and will be recreated on build.
2. **DLR** - The contents of the DLR (this) folder were taken from the 
   IronLanguages project (https://github.com/IronLanguages)
   version (IronLanguages-main-af33202 - _VERY OUTDATED_).
3. **docs** - Documentation files.
4. **obj** - Temporary object files and other files used during build.
   This folder can be safely deleted and will be recreated on build.
5. **src** - Contains the sources. 
   **NB:** This folder may never contain other files than source files! 
   Keeping it free from other files makes it easy to do file-level searches in the sources.
6. **src\Core** - Contains the ___core___ sources comprising the IronSmalltalk compilers and runtime.
7. **src\Test** - Contains ___unit tests___ to test projects in **src\Core**.
8. **src\Tools*** - Contains ___tools___ used during development. 
   Most importantly, this contains the **Class Library Browser**.

### IronSmalltalk Components (Assemblies) Overview: ###

* **Runtime Components written in .Net:**
	* Common (few classes shared between the compiler and the runtime)
	* Runtime (the core of IST; this is the brain and the heart)
	* Compiler (parses Smalltalk source code and creates ASTs)
	* Definition Installer (modifies the Smalltalk environment, e.g. creates clas, globals, methods etc.)
	* Interchange Installer (a definition installer that uses interchange files (*.ST files) to set up/modify the environment)
	* AST JIT Compiler (compiles ST ASTs to DLR ETs/Expressions)
	* Hosting (needed to host IST as language inside the DLR, but not necessary)

* **Smalltalk Class Library**
	* The standard class library - it doesn't need more introduction

* **Development Tools**
	* Class Hierarchy Browser / Editor (a tool written in .Net for the purpose of developing the class library - this is an internal dev. tool)
	* Test Playground (all type of experimental garbage)
	* Unit Tests

### Building Instructions ###
See [Build.md](Build.md).