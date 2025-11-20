import {
    List,
    Datagrid,
    TextField,
    DateField,
    ChipField,
    DeleteButton
} from 'react-admin';

export const CertificateList = () => (
    <List>
        <Datagrid rowClick="show" bulkActionButtons={false}>
            <TextField source="title" label="Certificate Title" />
            <TextField source="ownerName" label="Owner Name" />
            <TextField source="issuedBy" label="Issuer" />
            <DateField source="issuedDate" label="Issue Date" showTime />
            <ChipField source="blockchain" label="Blockchain" />
            <TextField
                source="certificateHash"
                label="Hash"
                sx={{
                    maxWidth: '150px',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap'
                }}
            />
            <DateField source="createdAt" label="Created At" showTime />
            <DeleteButton />
        </Datagrid>
    </List>
);