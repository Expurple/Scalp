# Scalp

An interpreter for my own simple scripting language, which I develop for fun and education.

Scalp is a basic statically typed C-like language, except for using new lines instead of semicolons, such as in Python.

## What's new in v0.1.0 (9 Aug 2020)?

* This is the first ever version of Scalp.
* It introduces an interactive Python-like interpreter, which supports:
* A type system (in which only *String* is present and user types can't be declared yet)
* Global variables, which can be assigned to a string literal or to another variable
* Functions aren't yet supported, but there is:
* A fake *print()* function which prints one string: a variable or a literal
* A fake *exit()* function which shuts down the interpreter
* And, sure, there are syntactical error checks.

## Versioning

I use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/Expurple/Scalp/tags). 

## Authors

* **Dmitry Alexandrov** - [Expurple](https://github.com/Expurple)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
