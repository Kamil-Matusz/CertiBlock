use polygon_db;

db.polygon.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });

db.polygon.createIndex({ "TransactionHash": 1 }, { "name": "TransactionHash_1_index" });

db.polygon.createIndex(
    { "Status": 1, "CertificateId": 1 },
    { "name": "Status_1_CertificateId_1_index" }
);

db.polygon_metrics.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });