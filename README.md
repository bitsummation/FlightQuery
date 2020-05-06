## Flight Query
Ability to query the FlightAware FlightXML 2 API using SQL syntax. 
![](https://user-images.githubusercontent.com/13210937/81029719-9d3e0400-8e4b-11ea-921c-1b74dac4a55c.png)
## Run
Application runs as a web app in docker that includes an editor.
To Run:
```
docker run -d -p 5001:80 --name flightquery bitsummation/flightquery
```
then go to
```
http://localhost:5001
```
Enter your username and API key. Type your query or click on the example links on the right side.

## Query
The queries follow the FlightXML2 documentation [here](https://flightaware.com/commercial/flightxml/explorer/). Each API is modeled as a table with the name matching the api call. The arguments to the api can be specified in the where clause or on joins.

The editor includes auto complete to help author queries.

Examples below.
#### Austin Airport Info
```sql
/*
* Airport Information for Austin
*/

select *
from airportinfo
where airportCode = 'kaus'
```
#### Enroute to Austin
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
#### Austin Scheduled Flights With Status
```sql
/*
* Current upcoming Austin departures
* joins to get flightid and uses the flightinfoex to get the status of the flight
* It uses a case statement to determine status
*/

select s.ident,
    e.origin,
    s.originCity,
    e.destination,
    s.destinationCity,
    s.filed_departuretime,
    s.estimatedarrivaltime,
    case
        when e.actualarrivaltime = -1 and e.actualdeparturetime = -1 and e.estimatedarrivaltime = -1
            then 'cancelled'
        when e.actualdeparturetime != 0 and e.actualarrivaltime = 0
            then 'enroute'
        when e.actualdeparturetime != 0 and e.actualarrivaltime != 0 and e.actualdeparturetime != e.actualarrivaltime
            then 'arrived'
        else 'not departed'
        end as status
from scheduled s
join getflightid f on f.departureTime = s.filed_departuretime and f.ident = s.ident
join flightinfoex e on e.faFlightID = f.faFlightID
where airport = "kaus" and filter = "airline"
```
#### Track 1 Flight
```sql
/*
* Queries enroute for flights enroute to Austin.  
* Takes 1 result from the inner query and joins to get 
* the historical track of the flight. Results 
* should update if flight is in progress.
*/

select e.ident,
    filed_departuretime,
    departureTime,
    origin,
    destination,
    h.timestamp,
    h.altitude,
    h.groundspeed,
    h.latitude,
    h.longitude
from (
    select ident, filed_departuretime
    from (
        select ident, filed_departuretime
        from enroute e
        where airport = 'kaus' and actualdeparturetime != 0 and filter = 'airline'
    ) e
    limit 1
) e
join inflightinfo i on i.ident = e.ident
join gethistoricaltrack h on h.faFlightID = i.faFlightID
```
## Call Programmatically
To send a query from code, post the SQL text and authorization header to:
```
http://localhost:5001/query
```
