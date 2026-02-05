import {
    Show,
    SimpleShowLayout,
    TextField,
    DateField,
    ChipField,
} from 'react-admin';

export const PolygonTransactionShow = () => (
    <Show>
        <SimpleShowLayout>
            <TextField source="id" label="ID" />
            <TextField source="certificateId" label="ID Certyfikatu" />
            <TextField
                source="transactionHash"
                label="Hash Transakcji"
                sx={{
                    fontFamily: 'monospace',
                    wordBreak: 'break-all',
                    fontSize: '0.9rem'
                }}
            />
            <ChipField source="status" label="Status" />
            <DateField source="createdAt" label="Data utworzenia" showTime />
        </SimpleShowLayout>
    </Show>
);