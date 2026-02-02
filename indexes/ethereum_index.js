use ethereum_db;

db.ethereum.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });

db.ethereum.createIndex({ "TransactionHash": 1 }, { "name": "TransactionHash_1_index" });

db.ethereum.createIndex(
    { "Status": 1, "CertificateId": 1 },
    { "name": "Status_1_CertificateId_1_index" }
);

db.ethereum_metrics.createIndex({ "CertificateId": 1 }, { "name": "Certificate_1_index" });