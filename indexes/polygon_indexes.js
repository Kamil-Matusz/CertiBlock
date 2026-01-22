use polygon_db;

db.polygon.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });

db.polygon_metrics.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });