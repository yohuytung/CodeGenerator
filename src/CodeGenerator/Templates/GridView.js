/**
 * 模块数据的主显示区域，继承自Grid
 */
Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}Grid', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.{{ModelNameLower}}Grid',
    uses: ['{{Company}}.model.{{Category}}.{{ModelName}}'],
    columnLines: true, // 加上表格线
    multiSelect: true,
    selModel: Ext.create('Ext.selection.CheckboxModel'), // 复选框
    title: '列表',
    listeners: {
        afterrender: 'onSearch',
        rowdblclick: 'onModify'//,
        // cellclick: "onCellClick"
    },
    initComponent: function() {
        this.store = Ext.create('Ext.data.JsonStore', {
            model: '{{Company}}.model.{{Category}}.{{ModelName}}',
            pageSize: 50,
            proxy: {
                type: 'ajax',
                url: '/api/{{ModelName}}',
                actionMethods: {
                    read: 'GET'
                },
                reader: {
                    type: 'json',
                    root: 'Data',
                    totalProperty: 'RecordCount',
                    idProperty: 'Id'
                }
            }
        });
        // 创建grid列
        this.columns = [{
            xtype: 'rownumberer',
            width: 35
        },{{ForeachFieldBegin}}
        {
            header: '{{FieldDesc}}',
            dataIndex: '{{FieldName}}',
            align: 'center',
            width: 150
        }{{ForeachFieldEnd}}
        ];

        this.dockedItems = [{
            xtype: 'gridtoolbar', // 按钮toolbar
            dock: 'top',
            items: [{
                text: '添加',
                iconCls: 'fa fa-plus fa-ext-correct',
                handler: 'onAdd'
            }, '-', {
                text: '修改',
                iconCls: 'fa fa-plus fa-ext-correct',
                handler: 'onModify'
            }, '-', {
                text: '删除',
                iconCls: 'fa fa-trash-o fa-ext-correct',
                handler: 'onRemove'
            }, '-', {
                xtype: 'button',
                iconCls: 'fa fa-check-square-o fa-ext-correct',
                text: '启用',
                handler: "onEnable"
            }, '-', {
                xtype: 'button',
                iconCls: 'fa fa-square-o fa-ext-correct',
                text: '禁用',
                handler: "onDisable"
            }]
        }, {
            xtype: 'pagingtoolbar',
            dock: 'bottom',
            store: this.store
        }];

        this.callParent();
    }
})