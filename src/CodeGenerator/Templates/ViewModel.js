Ext.define('{{Company}}.view.{{Category}}.{{ModelName}}Model', {
	extend: 'Ext.app.ViewModel',
	alias: 'viewmodel.{{ModelNameLower}}',
	data: {
		//搜索器值
		searcher: {
			mnemonicCode: ''
		},
		{{ModelNameLower}}: Ext.create("{{Company}}.model.{{Category}}.{{ModelName}}").data
	}
});