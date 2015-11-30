# Airy.Identity.Nhibernate

## About ##
NHibenrate implementation of the backing stores for ASP.Net Identity that can easily be used to replace the default Entity Framework provider.

## Features ##
* Drop-in replacement for ASP.Net Identity Entity Framework (v2.2.1) backing store that ships with ASP.Net MVC 5.
* By default matches with database design used by the Microsoft.AspNet.Identity.EntityFramework provider.
* Supports the use of types other than strings for primary keys such as Guids or integers.
* Supports additional properties on the User, Role, Login and Claim classes if required.
* Fully implements all of the UserStore and RoleStore interfaces.
* Sample ASP.Net MVC 5 project.

## Quick Start ##

## Acknowledgements ##
There is already an excellent official [NHibernate Implementation] (https://github.com/nhibernate/NHibernate.AspNet.Identity) of the ASP.Net Identity.
However it does not currently allow the use of types other than strings for primary keys [Issue 46] (https://github.com/nhibernate/NHibernate.AspNet.Identity/issues/46)  
As I needed to be able to use Guids for some primary keys I forked it and started to see how I could implement it.

It was a lot harder than I thought especially since I'm not an NHibernate Guru.  I found that I needed to take a different approach to the current implementation and decided it would be easier to do if I started from scratch with a new solution.  
I leant heavily on both the existing [official NHibernate Implementation] (https://github.com/nhibernate/NHibernate.AspNet.Identity) and also the [Microsoft Entity Framework Implementation] (https://aspnetidentity.codeplex.com/)  
So a huge thank you to those authors particularly https://github.com/milesibastos for their work and for open sourcing it, without them I would never had been able to complete this, my first open source project.
