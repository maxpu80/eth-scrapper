
beware cloudfront cache errors for russia

https://www.elastic.co/guide/en/elasticsearch/reference/current/run-elasticsearch-locally.html

Digest: sha256:061c2797184203f78f6809e5853196f0769f528b4e2fe06b73c8c63e146b1e49

```

-------------------------------------------------------------------------------------------------------------------------------------------------------------------
-> Elasticsearch security features have been automatically configured!
-> Authentication is enabled and cluster connections are encrypted.

->  Password for the elastic user (reset with `bin/elasticsearch-reset-password -u elastic`):
  MliLmELKmsCJSBU=lUMy

->  HTTP CA certificate SHA-256 fingerprint:
  4b17b197a9cc25b6dab263252138a0ccf8f28f981cb76885b14873aaae09feaf

->  Configure Kibana to use this cluster:
* Run Kibana and click the configuration link in the terminal when Kibana starts.
* Copy the following enrollment token and paste it into Kibana in your browser (valid for the next 30 minutes):
  eyJ2ZXIiOiI4LjQuMSIsImFkciI6WyIxNzIuMTkuMC4yOjkyMDAiXSwiZmdyIjoiNGIxN2IxOTdhOWNjMjViNmRhYjI2MzI1MjEzOGEwY2NmOGYyOGY5ODFjYjc2ODg1YjE0ODczYWFhZTA5ZmVhZiIsImtleSI6ImFzcXlHSU1CSlQ0SjdIV2hRZE5kOjM4cFhDQ0hrUzNta0R2ZkppcTBvMGcifQ==

-> Configure other nodes to join this cluster:
* Copy the following enrollment token and start new Elasticsearch nodes with `bin/elasticsearch --enrollment-token <token>` (valid for the next 30 minutes):
  eyJ2ZXIiOiI4LjQuMSIsImFkciI6WyIxNzIuMTkuMC4yOjkyMDAiXSwiZmdyIjoiNGIxN2IxOTdhOWNjMjViNmRhYjI2MzI1MjEzOGEwY2NmOGYyOGY5ODFjYjc2ODg1YjE0ODczYWFhZTA5ZmVhZiIsImtleSI6ImJNcXlHSU1CSlQ0SjdIV2hRZE5pOndQd1FMS09aUzhDLUVqcFI1YnY2bncifQ==

  If you're running in Docker, copy the enrollment token and run:
  `docker run -e "ENROLLMENT_TOKEN=<token>" docker.elastic.co/elasticsearch/elasticsearch:8.4.1`
-------------------------------------------------------------------------------------------------------------------------------------------------------------------
```