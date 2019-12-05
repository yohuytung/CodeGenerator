Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}', {
    extend: 'Ext.container.Container',
    alias: 'widget.{{ModelNameLower}}Panel',
    requires: [
        '{{Company}}.model.{{Category}}.{{ModelName}}',
        '{{Company}}.view.{{Category}}.{{ModelName}}Controller',
        '{{Company}}.view.{{Category}}.{{ModelName}}Model'
    ],
    uses: [
        '{{Company}}.view.{{Category}}.{{ModelName}}Grid',
        '{{Company}}.view.{{Category}}.{{ModelName}}Add'
    ],
    controller: '{{ModelNameLower}}',
    viewModel: {
        type: '{{ModelNameLower}}'
    },
    layout: 'border',
    plain: true,
    closable: true,
    closeAction: 'hide',
    border: false,
    title: '功能模块名',
    listeners: {
        afterRender: function(thisForm, options) {
            this.keyNav = Ext.create('Ext.util.KeyNav', this.el, {
                enter: function() {
                    Ext.getCmp('{{ModelNameLower}}-ButtonSearch').fireHandler();
                },
                scope: this
            });
        }
    },
    initComponent: function() {
        this.items = [{
            xtype: 'panel',
            layout: 'column',
            height: 35,
            border: true,
            columnWidth: 1,
            defaults: {
                labelWidth: 35,
                anchor: '98%',
                "padding": '2px'
            },
            region: 'north',
            margins: {
                top: 5,
                right: 5,
                bottom: 5,
                left: 5
            },
            bodyStyle: {
                "background-color": 'transparent'
            },
            items: [{
                xtype: 'textfield',
                tabIndex: 1,
                labelWidth: 80,
                fieldLabel: '查询字段',columnWidth: 0.2,
                bind: '{searcher.mnemonicCode}'
            }, {
                xtype: 'container',
                columnWidth: 0.2,
                border: false,
                defaults: {
                    anchor: '95%'
                },
                items: [{
                    xtype: 'container',
                    columnWidth: 0.5,
                    layout: 'column',
                    border: false,
                    items: [{
                        xtype: 'button',
                        text: '查询',
                        width: 50,
                        style: 'margin:0 10px 0 0',
                        handler: 'onSearch',
                        id: '{{ModelNameLower}}-ButtonSearch'
                    }, {
                        xtype: 'button',
                        text: '重置',
                        width: 50,
                        handler: 'onReset'
                    }]
                }]
            }]
        }, {
            xtype: '{{ModelNameLower}}Grid',
            id: '{{ModelNameLower}}Grid',
            region: 'center',
            margins: {
                top: 5,
                right: 5,
                bottom: 5,
                left: 0
            }
        }]
        this.callParent();
    }
});