use sco_db_s2025;

-- task 1
SELECT onum, amt, odate FROM orders;

-- task 2
SELECT * FROM customers WHERE snum = 1001;

-- task 3
SELECT city, sname, snum, comm FROM salespeople;

-- task 4
SELECT snum FROM orders
GROUP BY snum;

-- task 5
SELECT sname, city FROM salespeople
WHERE comm > 0.1 AND city = 'London';

-- task 6
SELECT * FROM Customers
WHERE rating <= 100 OR City = 'Rome';

-- task 7
SELECT *
FROM orders
WHERE odate IN ('2014-10-03','2014-10-04');

SELECT *
FROM orders
WHERE odate BETWEEN '2014-10-03' AND '2014-10-04';

-- task 8
SELECT *
FROM customers
WHERE LEFT(cname, 1) BETWEEN 'A' AND 'G';

-- task 9
SELECT *
FROM customers
WHERE UPPER(cname) LIKE 'C%';

-- task 10
SELECT *
FROM orders
WHERE amt = 0
   OR amt IS NULL;

-- task 11

SELECT
  COUNT(*) AS order_count
FROM orders
WHERE odate = '2014-10-03';

-- task 12
SELECT 
  COUNT(DISTINCT city) AS 'Distinct Cities'
FROM customers;

-- task 13
SELECT cname AS 'First Customer'
FROM customers
WHERE cname LIKE 'G%'
ORDER BY cname ASC
LIMIT 1;

-- task 14
SELECT *
FROM orders
WHERE odate IN ('2014-10-03','2014-10-04')
   OR dcity = 'Barcelona';

-- task 15
-- Method 1: directly on the orders table
SELECT 
  onum, 
  amt, 
  dcity AS city
FROM orders
WHERE dcity IN ('London','Barcelona')
  AND amt > 3000
  AND amt < 5000;
