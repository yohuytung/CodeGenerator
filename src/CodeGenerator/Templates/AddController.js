Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}AddController', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.{{ModelNameLower}}Add',
    onInit:function()
    {
    },  
    onReset: function() {
        // 这里很关键,需要重新创建MODEL   
        var viewModel = this.getView().getViewModel();
        var model = Ext.create("{{Company}}.model.{{Category}}.{{ModelName}}").data;
        viewModel.set("{{ModelNameLower}}", model);
    },
    onShow: function() {
        //加载数据
        var me = this;
        var viewModel = this.getView().getViewModel();
        var id = viewModel.get("{{ModelNameLower}}.Id");
        // 说明是新增，通过清空控件值
        if (id == 0) {
            this.onReset();
        } else {
            {{Company}}.util.Common.waitLoad();
            Ext.Ajax.request({
                url: '/api/{{ModelName}}/'+id,
                method: 'GET',
                success: function(r) {
                    var m = Ext.decode(r.responseText);
                    viewModel.set("{{ModelNameLower}}", m);
                    {{Company}}.util.Common.endLoad();
                },
                failure: function(e) {
                    {{Company}}.util.Common.communicationError();
                }
            });
        }
    },
    onSave: function() {
        this.onSaveInternal(1);
    },
    onSaveClose: function() {
        this.onSaveInternal(2);
    },
    onSaveContinue: function() {
        this.onSaveInternal(3);
    },
    onSaveInternal: function(saveType) {
        var me = this;
        var form = this.getView().down("form").getForm();
        if (form.isValid()) {
            var viewModel = this.getView().getViewModel();
            {{Company}}.util.Common.waitSubmit();

            var newMethod = viewModel.get('{{ModelNameLower}}.Id') > 0 ? "PUT" : "POST";

            Ext.Ajax.request({
                url: '/api/{{ModelNameLower}}',
                method: newMethod,
                jsonData: viewModel.get("{{ModelNameLower}}"),
                success: function(r) {
                    var m = Ext.decode(r.responseText);
                    if (m.Code == "000000") {
                        if (newMethod == "POST") {
                            viewModel.set("{{ModelNameLower}}.Id", m.Data.Id);
                        }
                        if (saveType == 2) {
                            me.getView().hide();
                        } else if (saveType == 3) {
                            me.onReset();
                        }
                        //刷新列表页面
                        var listController = Ext.getCmp('{{ModelNameLower}}Panel').getController();
                        listController.onSearch();
                        {{Company}}.util.Common.showSaveSuccess();
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

    }
});