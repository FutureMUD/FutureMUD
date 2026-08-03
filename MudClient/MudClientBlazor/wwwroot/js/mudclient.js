(function () {
	let sendCommandHelper = null;

	window.registerSendCommandHandler = function (dotNetHelper) {
		sendCommandHelper = dotNetHelper;
	};

	window.disposeSendCommandHandler = function () {
		sendCommandHelper = null;
	};

	document.addEventListener('click', function (event) {
		const link = event.target.closest && event.target.closest('[data-mxp-command-id]');
		if (!link) {
			return;
		}

		event.preventDefault();
		const commandId = Number.parseInt(link.dataset.mxpCommandId, 10);
		if (sendCommandHelper && Number.isInteger(commandId)) {
			sendCommandHelper.invokeMethodAsync('SendCommandById', commandId);
		}
	});

	window.mudClientHotkeys = (function () {
		let dotNetHelper = null;
		let keydownHandler = null;
		let enabled = true;
		let boundCodes = new Set();

		function isNumpadCode(code) {
			return code && code.indexOf('Numpad') === 0;
		}

		function isPrintableKey(event) {
			return typeof event.key === 'string' && event.key.length === 1;
		}

		function isEditableTarget(target) {
			return !!(target && target.closest && target.closest('input, textarea, select, [contenteditable]'));
		}

		function shouldIgnoreEvent(event, code) {
			if (!enabled || event.defaultPrevented || event.ctrlKey || event.altKey || event.metaKey || event.shiftKey) {
				return true;
			}

			const target = event.target;
			if (isEditableTarget(target) && isPrintableKey(event) && !isNumpadCode(code)) {
				return true;
			}

			return !!(target && target.closest && target.closest('[data-hotkey-settings="true"]'));
		}

		function handleKeydown(event) {
			const code = event.code || '';
			if (!code || shouldIgnoreEvent(event, code) || !boundCodes.has(code)) {
				return;
			}

			event.preventDefault();
			event.stopPropagation();
			if (event.stopImmediatePropagation) {
				event.stopImmediatePropagation();
			}

			if (dotNetHelper) {
				dotNetHelper.invokeMethodAsync('HandleHotkeyCode', code);
			}
		}

		return {
			register: function (helper) {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				dotNetHelper = helper;
				keydownHandler = handleKeydown;
				document.addEventListener('keydown', keydownHandler, true);
			},
			setBoundCodes: function (codes) {
				boundCodes = new Set(codes || []);
			},
			setEnabled: function (value) {
				enabled = !!value;
			},
			dispose: function () {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				keydownHandler = null;
				dotNetHelper = null;
				boundCodes = new Set();
			}
		};
	})();

	window.mudClientInput = (function () {
		let element = null;
		let handler = null;

		return {
			register: function (elementId) {
				this.dispose();
				element = document.getElementById(elementId);
				if (!element) {
					return;
				}

				handler = function (event) {
					const shouldPrevent =
						(event.key === 'Enter' && !event.shiftKey) ||
						event.key === 'ArrowUp' ||
						event.key === 'ArrowDown' ||
						(event.ctrlKey && (event.key === 'Home' || event.key === 'End'));
					if (shouldPrevent) {
						event.preventDefault();
					}
				};
				element.addEventListener('keydown', handler);
			},
			dispose: function () {
				if (element && handler) {
					element.removeEventListener('keydown', handler);
				}

				element = null;
				handler = null;
			}
		};
	})();

	window.scrollToBottomIfNearBottom = function (element) {
		if (!element) {
			return;
		}

		const previousHeight = Number.parseFloat(element.dataset.previousScrollHeight || '0');
		const referenceHeight = previousHeight > 0 ? previousHeight : element.scrollHeight;
		const wasNearBottom = referenceHeight - element.clientHeight - element.scrollTop < 160;
		element.dataset.previousScrollHeight = String(element.scrollHeight);
		if (wasNearBottom || previousHeight === 0) {
			window.requestAnimationFrame(function () {
				element.scrollTop = element.scrollHeight;
				element.dataset.previousScrollHeight = String(element.scrollHeight);
			});
		}
	};

	window.getSelectionStart = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionStart : 0;
	};

	window.getSelectionEnd = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionEnd : 0;
	};

	window.setSelectionRange = function (elementId, startPosition, endPosition) {
		const element = document.getElementById(elementId);
		if (!element) {
			return;
		}

		element.focus();
		const end = typeof endPosition === 'number' ? endPosition : startPosition;
		element.setSelectionRange(startPosition, end);
	};

	window.triggerDownload = function (filename, dataUri) {
		const link = document.createElement('a');
		link.href = dataUri;
		link.download = filename;
		link.click();
	};
})();
