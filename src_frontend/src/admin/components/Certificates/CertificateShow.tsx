import {
    Show,
    SimpleShowLayout,
    TextField,
    DateField,
} from 'react-admin';

export const CertificateShow = () => (
    <Show>
        <SimpleShowLayout>
            <TextField source="title" label="Certificate Title" />
            <TextField source="ownerName" label="Owner Name" />
            <TextField source="issuedBy" label="Issuer" />
            <DateField source="issuedDate" label="Issue Date" showTime />
            <TextField
                source="certificateHash"
                label="Hash certyfikatu"
                sx={{ fontFamily: 'monospace', wordBreak: 'break-all' }}
            />
            <DateField source="createdAt" label="Created At" showTime />
        </SimpleShowLayout>
    </Show>
);