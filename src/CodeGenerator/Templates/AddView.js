Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}Add', {
    extend: '{{Company}}.view.widget.BaseWindow',
    alias: 'widget.{{ModelNameLower}}AddPanel',
    requires: [
        '{{Company}}.view.{{Category}}.{{ModelName}}AddController'
    ],
    controller: '{{ModelNameLower}}Add',
    viewModel: {
        type: '{{ModelNameLower}}'
    },
    width: 800,
    height: 600,
    layout: 'border',
    plain: true,
    border: false,
    defaults: {
        margin: '5px'
    },
    title: '添加/修改',
    listeners: {
        afterrender: 'onInit',
        show: 'onShow'
    },
    initComponent: function() {
        this.items = [{
            xtype: 'baseform',
            layout: "fit",
            region: 'center',
            items: [{
                xtype: 'panel',
                region: 'center',
                layout: 'border',
                border: false,
                // columnWidth: 0.8,
                items: [{
                    xtype: 'panel',
                    border: false,
                    region: 'north',
                    bodyPadding: '0 0 0 0',
                    layout: 'column',
                    bodyStyle: {
                        "background-color": 'transparent'
                    },
                    defaults: {
                        columnWidth: 1,
                        labelWidth: 70
                    },
                    items: [{{ForeachFieldBegin}}
                    {
                        xtype: 'textfield',
                        fieldLabel: '{{FieldDesc}}',
                        //maxLength: {{MaxLength}},
                        columnWidth: 0.5,
                        allowBlank: {{IsRequied}},
                        bind: '{{ModelNameLower}}.{{FieldName}}'
                    }{{ForeachFieldEnd}}]
                }]
            }]
        }];
        this.callParent();
    }
});