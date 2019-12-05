/**+++++++++++++++++++++++++++
 * 说明：{{ModelDesc}}
 *
 * 编写：[自动生成]
 * 创建：{{CreateDate}}
 * 修改：{{CreateDate}}
 *+++++++++++++++++++++++++++*/
{{Company}}.{{Category}}.{{ModelName}} = function() {
    var storeGrid,  // 列表数据源
        cmGrid,     // 列表列头
        grid,       // 列表
        fnInit,     // 初始化
        fnDel,      // 删除
        fnEdit,     // 编辑
        fnAdd,      // 添加
        fnSave,     // 保存
        fnClearFilter,//清除store过滤
        fnGetQueryCondition,//获取列表查询条件
        fnExportExcel, // 列表数据导出
        winEdit,    // 编辑窗口
        panelView;  // 页面主面板
    /**
     * 清除store过滤
     */
    fnClearFilter = function () {
        // store.clearFilter();
    };
    /**
     * 获取列表查询条件
     */
    fnGetQueryCondition = function () {
        // 日期字段转换: Ext.util.Format.date(Ext.getCmp('FieldName').getValue(), "Y-m-d") || null
        var conditionParams = {{{ForeachFieldBegin}}            
            {{FieldName}}: Ext.getCmp('{{ModelName}}-search-{{FieldName}}').getValue() || null{{ForeachFieldEnd}}
        };
        return conditionParams;
    };
    
    /**
    * 导出列表数据Excel
    */
    fnExportExcel = function (type) {
        if (storeGrid.getCount() == 0) {
            ZdtCore2.utiliy.showMsgBox("当前没有可导出的数据！", 'error');
            return;
        } else {
            var condition = fnGetQueryCondition();
            var urlParams = ['type=' + type];
            for (var p in condition) {
                var v = condition[p];
                urlParams.push(p + '=' + (v != null ? v : ''));
            }
            var downLoadUrl = "/api/Export{{ModelName}}?" + urlParams.join("&");
            window.open(downLoadUrl);
        }
    };
    /**
     * [初始化]
     * @return {[type]} [description]
     */
    fnInit = function() {
        // TODO
        // description
        fnClearFilter();
    };
    /**
     * 保存
     * @param  {int} closeType [关闭类型]
     * @return {[type]} [description]
     */
    fnSave = function(closeType) {
        if (winEdit.isAdd && !ZdtCore2.Core.checkPermiss('{{ModelName}}-Add') || !ZdtCore2.Core.checkPermiss('{{ModelName}}-Modif')) {
            return;
        }

        if (ZdtCore2.utiliy.callValid(winEdit)) {            
            var md = {
                // Id: winEdit.isAdd ? 0 : winEdit.{{ModelName}}.Id,{{ForeachFieldBegin}}
            {{FieldName}}: Ext.getCmp('{{ModelName}}-{{FieldName}}').getValue(){{ForeachFieldEnd}}
        };

        ZdtCore2.utiliy.showWaitBox();
        Ext.Ajax.request({
            url: "/api/{{ModelName}}",
            method: winEdit.isAdd ? 'POST' : 'PUT',
            jsonData: md,
            success: function(r) {
                var m = Ext.decode(r.responseText);
                if (m.IsSuccess) {
                    if (closeType == 1) { // 保存并关闭
                        winEdit.hide();
                    } else if (closeType == 2) { // 保存并继续
                        ZdtCore2.utiliy.resetValue(winEdit);
                        winEdit.{{ModelName}} = null;
                        winEdit.isAdd = true;
                        fnInit();
                    } else if (winEdit.isAdd) {
                        winEdit.isAdd = false;
                        winEdit.{{ModelName}} = m.Data;
                        // Ext.getCmp('{{ModelName}}-No').setValue(m.Data.No); // 如果有编号生成，请赋值
                    }
                    grid.getSelectionModel().deselectAll();
                    storeGrid.load();
                    ZdtCore2.utiliy.showMsgBox("保存成功！", "info");
                }
                else
                {
                    ZdtCore2.utiliy.showMsgBox(m.Msg, "error");
                }
            },
            failure: function() {
                ZdtCore2.utiliy.showMsgBox('与服务器通信异常，添加/修改失败！', "error");
            }
        });
    }
};
/**
 * 编辑
 * @return {[type]} [description]
 */
fnEdit = function() {
    var sm = grid.getSelectionModel();
    if (sm.getCount() < 1) {
        ZdtCore2.utiliy.showMsgBox('请选择要修改的数据行！', "error");
        return;
    }
    fnInit();
    var m = sm.getSelection()[0].data;
    winEdit.{{ModelName}} = m;
    winEdit.isAdd = false;
    {{ForeachFieldBegin}}
    Ext.getCmp('{{ModelName}}-{{FieldName}}').setValue(m.{{FieldName}});//{{ForeachFieldEnd}}

winEdit.show();
};
/**
 * 添加
 * @return {[type]} [description]
 */
fnAdd = function() {
    if (ZdtCore2.Core.checkPermiss('{{ModelName}}-Add')) {
        winEdit.{{ModelName}} = null;
        winEdit.isAdd = true;
        ZdtCore2.utiliy.resetValue(winEdit);
        fnInit();
        winEdit.show();
    }
};
/**
 * 删除
 * @return {[type]} [description]
 */
fnDel = function() {
    if (!ZdtCore2.Core.checkPermiss('{{ModelName}}-Delete')) {
        return;
    }

    var sm = grid.getSelectionModel();
    if (sm.getCount() < 1) {
        ZdtCore2.utiliy.showMsgBox('请选择要删除的数据行！', "error");
        return;
    }

    //执行删除
    var fnConfirm = function() {
        ZdtCore2.utiliy.showWaitBox();
        Ext.Ajax.request({
            url: '/api/{{ModelName}}/' + sm.getSelection()[0].data.Id,
            method: 'DELETE',
            // jsonData: [sm.getSelection()[0].data.Id],
            success: function(r) {
                var m = Ext.decode(r.responseText);
                if (!m.IsSuccess) {
                    ZdtCore2.utiliy.showMsgBox(m.Msg, "error");
                } else {
                    ZdtCore2.utiliy.showMsgBox("删除成功！", "info");
                    storeGrid.load();
                }
            },
            failure: function() {
                ZdtCore2.utiliy.showMsgBox('删除失败，服务器通信异常！', "error");
            }
        });
    };
    ZdtCore2.utiliy.showMsgBox('删除后将无法恢复，确认继续？', 'confirm', fnConfirm);
};

storeGrid = Ext.create('Ext.data.JsonStore', {
    proxy: {
        type: 'ajax',
        url: '/api/{{ModelName}}',
        method: 'GET',
        reader: {
            type: 'json',
            root: 'Data',
            idProperty: 'Id',
            totalProperty: 'RecordCount'
        }
    },
    fields: [{{ForeachFieldBegin}}
    { name: '{{FieldName}}'{{FieldType}} } /* {{FieldDesc}} */{{ForeachFieldEnd}}
]
});

cmGrid = [
    {
    xtype: 'rownumberer'
}, {{ForeachFieldBegin}}
{{{IsDateFormat}}
header: '{{FieldDesc}}',
dataIndex: '{{FieldName}}',
align: 'center',
width: 150
}{{ForeachFieldEnd}}
];

grid = Ext.create('Ext.grid.Panel', {
    margins: 3,
    height: 300,
    columnLines: true,
    store: storeGrid,
    columns: cmGrid,
    region: 'center',
    tbar: [{
        text: '添加',
        iconCls: 'icon-Add16',
        tooltip: '添加{{ModelDesc}}',
        handler: fnAdd
    }, '-', {
        text: '修改',
        iconCls: 'icon-Edit16',
        tooltip: '修改选中的{{ModelDesc}}',
        handler: fnEdit
    }, '-', {
        text: '删除',
        iconCls: 'icon-Del16',
        tooltip: '删除选中的{{ModelDesc}}',
        handler: fnDel
    }, '-', {
        xtype: 'button',
        text: "导出Excel",
        iconCls: "icon-ExportExcel16",
        handler: function () {
            fnExportExcel(0);
        }
    }],
    bbar: ZdtCore2.utiliy.createPaging(storeGrid),
    listeners: {
        beforerender: function() {
            var p = fnGetQueryCondition();
            storeGrid.load({
                params: p
            });
},
celldblclick: function() {
    fnEdit();
}
}
});

winEdit = Ext.create('Ext.window.Window', {
    title: '{{ModelDesc}}',
    width: ZdtCore2.Core.MAXWIDTH,
    height: ZdtCore2.Core.MAXHEIGHT,
    plain: true,
    farme: false,
    border: false,
    closeAction: 'hide',
    items: [{
            xtype: 'container',
            layout: 'column',
            margin: '3 0 0 3',
            defaults: {
                columnWidth: 1 / 5,
                labelWidth: 70,
                margin: '0 3 3 0'
            },
            items: [{{ForeachFieldBegin}}
            {
                    {{FieldXType}}
        fieldLabel: '{{FieldDesc}}',
id: '{{ModelName}}-{{FieldName}}',
maxLength: {{MaxLength}},
allowBlank: {{IsRequied}}
}{{ForeachFieldEnd}}
]
}
],
buttonAlign: 'right',
    buttons: [{
        text: '保存',
        tabIndex: 14,
        handler: function() {
            fnSave(0);
        }
    }, {
        text: '保存并关闭',
        tabIndex: 15,
        handler: function() {
            fnSave(1);
        }
    }, {
        text: '保存并继续',
        tabIndex: 16,
        handler: function() {
            fnSave(2);
        }
    }, {
        text: '关闭',
        tabIndex: 17,
        style: 'padding:0 3 0 0',
        handler: function() {
            winEdit.hide();
        }
    }]
});

panelView = Ext.create('Ext.panel.Panel', {
    layout: 'border',
    resizable: false,
    border: false,
    plain: true,
    closable: true,
    closeAction: 'hide',
    title: '{{ModelDesc}}',
    iconCls: 'icon-Detail16',
    items: [{
        xtype: 'panel',
        layout: 'column',
        height: 70,
        border: true,
        region: 'north',
        margin: '3 3 0 3',
        bodyStyle: {
            "background-color": 'transparent',
            "padding": '3px 0px 0px 3px'
        },
        defaults: {
            columnWidth: 1 / 6,
            labelWidth: 55,
            margin: '0 3 3 0'
        },
        items: [{{ForeachFieldBegin}}
        {
                    {{FieldXType}}
    fieldLabel: '{{FieldDesc}}',
id: '{{ModelName}}-search-{{FieldName}}',
maxLength: {{MaxLength}}
}{{ForeachFieldEnd}},
{
    xtype: 'container',
    layout: 'column',
    width: 200,
    items: [{
        xtype: 'container',
        layout: 'column',
        border: false,
        items: [{
            xtype: 'button',
            id: '{{ModelName}}-search',
            text: '查询',
            width: 55,
            tooltip: '查询数据',
            margin: '0 8 0 0',
            handler: function() {
                var p = fnGetQueryCondition();
                storeGrid.load({
                    params: p
                });
}
}, {
    xtype: 'button',
    id: '{{ModelName}}-reset',
    text: '重置',
    tooltip: '重置查询条件',
    width: 55,
    handler: function() {
        ZdtCore2.utiliy.resetValue(panelView);
    }
}]
}]
}]
}, grid],
listeners: {
        afterrender: function() {
            ZdtCore2.utiliy.regHotKey(panelView, {
                search: '{{ModelName}}-search',
                reset: '{{ModelName}}-reset'
            });
        }
}
});

return {
    view: panelView,
    atype: '{{Company}}.{{Category}}.{{ModelName}}',
    baseCode: '{{ModelName}}-View'
};
};