## Flight Query
![](https://user-images.githubusercontent.com/13210937/81029719-9d3e0400-8e4b-11ea-921c-1b74dac4a55c.png)
## Run
Ability to query the flighaware xml 2 api using sql syntax. Application runs as a web app in docker that includes an editor.
To Run:
```
docker run -d -p 5001:80 --name flightquery bitsummation/flightquery
```
then go to
```
http://localhost:5001
```
Enter your username and apikey and type your query or click on the example links on the right side.
To send a query from code, post the sql text and authorization header to:
```
http://localhost:5001/query
```
## Query
The queries follow the FlightXML2 documentation [here](https://flightaware.com/commercial/flightxml/explorer/). Each API is modeled as a table with the name matching the api call. The arguments to the api can be specified in the where clause or on joins.

The editor includes auto complete.

Example below and more docs to follow.

```sql
/*
* Flights enroute to austin
*/
select e.ident,
    actualdeparturetime,
    filed_departuretime,
    estimatedarrivaltime,
    originCity,
    destinationCity,
    latitude,
    longitude,
    altitude
from enroute e
join inflightinfo i on e.ident = i.ident
where airport = 'kaus' and actualdeparturetime != 0 and filter = 'airline'

```
