
Cocop.MessageSerialiser.Meas (C#) v.2.0.0
=========================================

---

<img src="logos.png" alt="COCOP and EU" style="display:block;margin-right:auto" />

COCOP - Coordinating Optimisation of Complex Industrial Processes  
https://cocop-spire.eu/

This project has received funding from the European Union's Horizon 2020 
research and innovation programme under grant agreement No 723661. This piece 
of software reflects only the authors' views, and the Commission is not 
responsible for any use that may be made of the information contained therein.

---


Author
------

Petri Kannisto, Tampere University, Finland  
https://github.com/kannisto  
http://kannisto.org

**Please make sure to read and understand [LICENSE.txt](./LICENSE.txt)!**


COCOP Toolkit
-------------

This application is a part of COCOP Toolkit, which was developed to enable a 
decoupled and well-scalable architecture in industrial systems. Please see 
https://kannisto.github.io/Cocop-Toolkit/


Introduction
------------

This library provides a generic API to serialise and deserialise (or to encode
and decode) messages that contain measurement values and other data from 
industrial systems. Please familiarise yourself with this page: 
https://kannisto.github.io/Cocop-Toolkit/messageserialiser.html

All message structures are based on existing open standards. In this API, the 
standards have been created by and are copyrighted to the Open Geospatial 
Consortium, Inc. (OGC(r); https://www.ogc.org/). However, OGC is _not_ the
creator of this software, and this software has _not_ been certified for 
compliance with any OGC standard.

Because the utilised standards are complex and versatile, the API selectively
implements what is considered most important in industrial systems. To
guarantee interoperability, the API implements a _profile_ that covers a subset
of the standards. In addition, there are extensions not included in the
standards, but these are few.

The implemented features cover, for instance:

* measurement values of multiple types, such as:
  * numeric (floating point and integer)
  * boolean
  * enumeration (category)
  * time instant
  * time range
  * array
  * record of multiple values
* timeseries data (numeric)
* metadata of measurements
* remote control of long-running tasks
* request-response communication of measurements

This software has been implemented with C#/.NET. However, there is another 
implementation in Java (see 
https://github.com/kannisto/Cocop.MessageSerialiser.Meas_java). Still, because
the serialisation syntax is XML, the messages do not restrict which platform to
choose for implementation, as long as the messages comply with the
standards-based profile. That is, you could have one application in .NET,
another in Java and third in NodeJs, for instance, and these would be
completely interoperable.

This repository contains the following applications:

* MessageSerialiserMeas: the actual library
* (All other projects): test applications


Source Code and API Doc
-----------------------

* Github repo: https://github.com/kannisto/Cocop.MessageSerialiser.Meas_csharp
* API documentation: https://kannisto.github.io/Cocop.MessageSerialiser.Meas_csharp


Messages
--------

The following classes of this API can generate XML documents:

* _Observation_; this encloses a data item (_Item\_*_ classes) and associates
related metadata.
* _GetObservationRequest_, _GetObservationResponse_, _InsertObservationRequest_
and _InsertObservationResponse_; these can deliver observations in the
request-response manner.
* _TaskRequest_ and _TaskResponse_; these enable the remote control of
long-running tasks.
* _TaskStatusReport_; this notifies about task progress.

That is, all other objects must be located in any of the above to be included
in a message. Examples:

* To stream measurement data (_Item\_*_) in the publish-subscribe manner, you
would wrap each data item in an _Observation_.
* To explicitly retrieve data, you send a _GetObservationRequest_ and receive a
_GetObservationResponse_. In these, you again wrap your data (_Item\_*_) in an
_Observation_.

Please see the enclosed code examples.


Work of Third Parties
---------------------

This software distribution does not include any third-party modules, that is,
any of the utilised standards, the related XML schemata or any proxy software
generated from these schemata. The reason is that the authors are not lawyers
but software developers and the intention is to ensure that any copyright or
licensing issues are avoided. Even if it were possible to distribute the OGC
standards, these standards have dependencies to modules not copyrighted to the
OGC, which adds difficulty to licensing. However, because this software library
is largely based on the works of OGC, the OGC license conditions apply.

Still, because the third-party modules are necessary for this library to work,
there are instructions to retrieve them. You must download the required XML
schema files, generate the proxies and modify these as instructed. For more
information, please see the folder '\_codegen\_xsd\_exe'.


Environment
-----------

The development environment was _Microsoft Visual Studio 2017 (Version 15.8.2)_.

The .NET Framework version is the ancient 4.5. This was chosen to reduce the
risk that the application should be deployed in an environment that lacks a 
sufficiently new framework version.
