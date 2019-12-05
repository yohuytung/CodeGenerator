Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}Controller', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.{{ModelNameLower}}',
    onSearch: function() {
        var view = this.getView();
        var store = view.down("{{ModelNameLower}}Grid").getStore();
        var viewModel = view.getViewModel();

        if (store) {
            store.load({
                params: {
                    mnemonicCode: viewModel.get('searcher.mnemonicCode')
                }
            });
        }
    },
    onCellClick: function(me, td, cellIndex, record, tr, rowIndex, e, eOpts) {
        if (cellIndex == 2) {
            var enabled = !record.data.Enable;
            this.onEnablePut([record.data.Id], enabled);
        }
    },
    onEnable: function() {
        this.onEnableInternal(true);
    },
    onDisable: function() {
        this.onEnableInternal(false);
    },
    onEnableInternal: function(enable) {
        var me = this;
        var sm = this.getView().down('{{ModelNameLower}}Grid').getSelectionModel();
        if (sm.getCount() <= 0) {
            {{Company}}.util.Common.showWarning('请至少选择一行数据！');
            return;
        }
        var ids = [];

        Ext.each(sm.getSelection(), function(r, index) {
            ids.push(r.data.Id);
        });
        me.onEnablePut(ids, enable)
    },
    onEnablePut: function(ids, enabled) {
        var me = this;
        {{Company}}.util.Common.waitSubmit();
        Ext.Ajax.request({
            url: '/api/{{ModelName}}', ///'+id+'?enabled='+enabled,
            method: 'PUT',
            jsonData: Ext.encode(ids),
            params: {
                enable: enabled
            },
            success: function(r) {
                var m = Ext.decode(r.responseText);
                if (m.Code == "000000") {
                    {{Company}}.util.Common.endLoad();
                    me.onSearch();
                } else {
                    {{Company}}.util.Common.showError(m.Msg);
                    return;
                }
            },
            failure: function(e) {
                {{Company}}.util.Common.communicationError();
            }
        });
    },
    onReset: function() {
        var view = this.getView();
        {{Company}}.util.Common.clearValue(view);
    },
    onModify: function() {
        var sm = this.getView().down('{{ModelNameLower}}Grid').getSelectionModel();
        if (sm.getCount() != 1) {
            {{Company}}.util.Common.showWarning('请选择需要修改的数据行！');
            return;
        } else {
            var addWindow = Ext.getCmp('{{ModelNameLower}}AddPanel');
            if (!addWindow) {
                addWindow = Ext.widget('{{ModelNameLower}}AddPanel', {
                    id: '{{ModelNameLower}}AddPanel'
                });
            }
            var viewModel = addWindow.getViewModel();
            viewModel.set("{{ModelNameLower}}.Id", sm.getSelection()[0].data.Id);
            addWindow.show();
        }
    },
    onAdd: function() {
        //清空Model
        var addWindow = Ext.getCmp('{{ModelNameLower}}AddPanel');
        if (!addWindow) {
            addWindow = Ext.widget('{{ModelNameLower}}AddPanel', {
                id: '{{ModelNameLower}}AddPanel'
            });
        }
        var viewModel = addWindow.getViewModel();
        var createModel = Ext.create("{{Company}}.model.{{Category}}.{{ModelName}}").data;
        viewModel.set("{{ModelNameLower}}", createModel);
        addWindow.show();
    },

    onRemove: function() {
        var me = this;
        var sm = me.getView().down('{{ModelNameLower}}Grid').getSelectionModel();
        var remove = function() {
            var ids = []
            Ext.each(sm.getSelection(), function(r) {
                ids.push(r.data.Id);
            });
            {{Company}}.util.Common.waitSubmit();
            Ext.Ajax.request({
                url: '/api/{{ModelName}}',
                method: 'DELETE',
                jsonData: ids,
                success: function(r) {
                    var m = Ext.decode(r.responseText);
                    if (m.Code == "000000") {
                        // {{Company}}.util.Common.showDeleteSuccess();
                        //刷新列表页面
                        {{Company}}.util.Common.endLoad();
                        me.onSearch();
                    } else {
                        {{Company}}.util.Common.showError(m.Msg);
                        return;
                    }
                },
                failure: function(e) {
                    {{Company}}.util.Common.communicationError();
                }
            });
        }


        if (sm.getCount() <= 0) {
            {{Company}}.util.Common.showWarning('请选择需要删除的数据行！');
        } else {
            {{Company}}.util.Common.showConfirm("删除后将无法恢复，确认继续？", remove);
        }
    }
});