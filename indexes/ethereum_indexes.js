use ethereum_db;

db.ethereum.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });

db.ethereum_metrics.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });