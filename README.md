Ability to query the flighaware xml api using sql syntax. Application runs as a web app in docker that includes an editor.
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

Example below and more docs to follow.

```sql
select
	a.ident,
	a.departuretime,
	a.origin,
	a.destination,
	case
	when e.actualarrivaltime = -1 and e.actualdeparturetime = -1 and e.estimatedarrivaltime = -1
		then 'cancelled'
	when e.actualdeparturetime != 0 and e.actualarrivaltime = 0
		then 'enroute'
	when e.actualdeparturetime != 0 and e.actualarrivaltime != 0 and e.actualdeparturetime != e.actualarrivaltime
		then 'arrived'
	else 'not departed'
	end as status
from airlineflightschedules a
join getflightid f on f.departureTime = a.departuretime and f.ident = a.ident
join flightinfoex e on e.faFlightID = f.faFlightID
where a.departuretime > '2020-4-19 21:43' and a.origin = 'kaus'
```
