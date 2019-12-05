Ext.define('{{Company}}.model.{{Category}}.{{ModelName}}', {
    extend: 'Ext.data.Model',
    fields: [{{ForeachFieldBegin}}
        { name: '{{FieldName}}'{{FieldType}} } /* {{FieldDesc}} */{{ForeachFieldEnd}}
    ]
});